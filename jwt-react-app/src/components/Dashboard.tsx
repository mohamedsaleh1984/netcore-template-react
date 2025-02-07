import React, { useEffect, useState } from 'react';
import * as TokenUtil from "../Utils"
import { Client } from 'src/api/Client';
import { RefreshTokenRequest } from 'src/api/ApiClient';
import { jwtDecode } from 'jwt-decode';

const Dashboard: React.FC = () => {
    const [data, setData] = useState<any>(null);
    const [error, setError] = useState<string | null>(null);

    const handleLogout = async () => {
        try {
            // some logic
            TokenUtil.removeAccessToken();
            TokenUtil.removeRefreshToken();
            // Redirect to login
            window.location.href = '/login';
        } catch (error) {
            //console.error('Logout failed:', error.response?.data?.message || error.message);
        }
    }

    const refreshAccessToken = async () => {
        try {
            const reqParam: RefreshTokenRequest = { token: TokenUtil.getRefreshToken() };
            const response = await Client.auth_AuthRefreshToken(reqParam);
            const { accessToken, refreshToken } = response;
            TokenUtil.setAccessToken(accessToken);
            TokenUtil.setRefreshToken(refreshToken.token);

            return TokenUtil.getAccessToken();
        } catch (error) {
            console.error('Failed to refresh token', error);
            TokenUtil.removeAccessToken();
            TokenUtil.removeRefreshToken();
            window.location.href = '/login'; // Redirect to login
            return null;
        }
    };

    const validateToken = (token: string) => {
        try {

            // Decode the token
            const decodedToken: any = jwtDecode(token);

            // Check if the token is expired
            const currentTime = Date.now() / 1000; // Convert to seconds
            if (decodedToken.exp < currentTime) {
                return { isValid: false, error: "Token has expired." };
            }

            // If the token is valid, return the decoded payload
            return { isValid: true, decodedToken };
        } catch (error) {
            return { isValid: false, error: "Invalid token." };
        }
    };

    const fetchData = async () => {
        try {
            const response = await Client.protected_Get();
            setData(response);
        } catch (error: any) {
            console.log("WHATS UP")
            if (error.response && error.response.status === 401) {
                // Token expired, try refreshing
                const newAccessToken = await refreshAccessToken();
                if (newAccessToken) {
                    // Retry the request with the new token
                    const retryResponse = await Client.protected_Get();
                    setData(retryResponse);
                }
            } else {
                setError('Failed to fetch data');
            }
        }
    };

    const logic = async () => {
        // check if there is access token or not
        const accessToken = TokenUtil.getAccessToken();
        if (!accessToken) {
            window.location.href = '/login';
            return;
        }

        const valToken = validateToken(accessToken);
        if (!valToken.isValid) {
            const accessToken = await refreshAccessToken();
            if (accessToken) {
                await fetchData();
            }
        }


    }

    useEffect(() => {
        logic();
    }, []);

    if (error) {
        return <div>{error}</div>;
    }

    if (!data) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            <h1>Dashboard</h1>
            <pre>{JSON.stringify(data, null, 2)}</pre>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
};

export default Dashboard;