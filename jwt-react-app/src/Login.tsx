import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import * as TokenUtil from "./Utils"
import { Client } from './api/Client';
const g = require('./GLOBAL.json')

const Login: React.FC = () => {
    const [username, setUsername] = useState('admin');
    const [password, setPassword] = useState('admin');
    const navigate = useNavigate();

    const handleLogin = async () => {
        try {
            const response = await Client.auth_Login(username, password);

            const { accessToken, refreshToken } = response.data;

            TokenUtil.setAccessToken(accessToken);
            TokenUtil.setRefreshToken(refreshToken);

            navigate('/dashboard');
        } catch (error) {
            console.error('Login failed', error);
        }
    };

    return (
        <div>
            <h1>Login</h1>
            <input type="text" placeholder="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
            <input type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} />
            <button onClick={handleLogin}>Login</button>
        </div>
    );
};

export default Login;