import React, { useEffect, useState } from 'react';
import * as TokenUtil from "../Utils"
import { Client } from 'src/api/Client'; 
import { jwtDecode } from 'jwt-decode';
import { RefreshTokenRequestDto } from 'src/api/ApiClient';

 
const Dashboard: React.FC = () => {
    const [data, setData] = useState<any>(null);
    const [error, setError] = useState<string | null>(null);

    const getUserId = ()=>{
        const token = TokenUtil.getAccessToken();
        try {
            const decodedToken = jwtDecode(token);
            const userId = decodedToken.sub; // Common claims for user ID
            return userId;
        } catch (error) {
            console.error('Error decoding token:', error);
        }
        return "NA";
    }

    const handleLogout = async () => {
        try {
            TokenUtil.Inspect("handleLogout", "route...")
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
            
            var refToken:RefreshTokenRequestDto= {
                userId:getUserId()!,
                refreshToken:TokenUtil.getRefreshToken()
            }
            
            const response = await Client.auth_RefreshToken(refToken);
            const { accessToken, refreshToken } = response;

            TokenUtil.setAccessToken(accessToken);
            TokenUtil.setRefreshToken(refreshToken);
            
            TokenUtil.Inspect("refreshAccessToken", [accessToken, accessToken])
            
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
            TokenUtil.Inspect('validateToken', decodedToken)

            // Check if the token is expired
            const currentTime = Date.now() / 1000; // Convert to seconds

            if (decodedToken.exp < currentTime) {
                TokenUtil.Inspect('validateToken', "Token has expired.")
                return { isValid: false, error: "Token has expired." };
            }

            TokenUtil.Inspect('validateToken', "Token is not expired.")
            // If the token is valid, return the decoded payload
            return { isValid: true, decodedToken };
        } catch (error) {
            return { isValid: false, error: "Invalid token." };
        }
    };

    const fetchData = async () => {
        try {
            const response = await Client.protected_Call();
            setData(response);
        } catch (error: any) {
            TokenUtil.Inspect("fetchData", "Failed to fetchData");

            if (error.response && error.response.status === 401) {
                // Token expired, try refreshing
                const newAccessToken = await refreshAccessToken();
                if (newAccessToken) {
                    // Retry the request with the new token
                    const retryResponse = await Client.protected_Call();
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

        TokenUtil.Inspect("logic", accessToken);

        const valToken = validateToken(accessToken);
        if (!valToken.isValid) {
            const accessToken = await refreshAccessToken();
            if (accessToken) {
                await fetchData();
            }
        } else {
            TokenUtil.Inspect("logic", "Access Token is valid");
            await fetchData();
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