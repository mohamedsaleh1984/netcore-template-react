import React, { useEffect, useState } from 'react';
import axios from 'axios';
import * as TokenUtil from "./Utils"
const g = require('./GLOBAL.json')

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

            const response = await axios.post(g.baseUrl + '/refresh-token', { token: TokenUtil.getRefreshToken() });
            const { accessToken, refreshToken } = response.data;
            TokenUtil.setAccessToken(accessToken);
            TokenUtil.setRefreshToken(refreshToken);

            return TokenUtil.getAccessToken();
        } catch (error) {
            console.error('Failed to refresh token', error);
            TokenUtil.removeAccessToken();
            TokenUtil.removeRefreshToken();
            window.location.href = '/login'; // Redirect to login
            return null;
        }
    };

    const fetchData = async () => {
        try {
            const response = await axios.get(g.baseUrl + '/Protected', {
                headers: {
                    Authorization: `Bearer ${TokenUtil.getAccessToken()}`,
                },
            });
            setData(response.data);
        } catch (error: any) {
            console.log("WHATS UP")
            if (error.response && error.response.status === 401) {
                // Token expired, try refreshing
                const newAccessToken = await refreshAccessToken();
                if (newAccessToken) {
                    // Retry the request with the new token
                    const retryResponse = await axios.get(g.baseUrl + '/Protected', {
                        headers: {
                            Authorization: `Bearer ${newAccessToken}`,
                        },
                    });
                    setData(retryResponse.data);
                }
            } else {
                setError('Failed to fetch data');
            }
        }
    };

    useEffect(() => {
        fetchData();
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