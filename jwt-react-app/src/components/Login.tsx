import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import * as TokenUtil from "../Utils"
import { Client } from '../api/Client';
import { UserDto } from 'src/api/ApiClient';

const Login: React.FC = () => {
    const [username, setUsername] = useState('admin');
    const [password, setPassword] = useState('password');
    const navigate = useNavigate();

    const handleRegister = async()=>{
        var userInfo:UserDto = {password,username};
        const response = await Client.auth_Register(userInfo);
        console.log(response.id);
        console.log(response.passwordHash);
        console.log(response.role);
        console.log(response.username);
        TokenUtil.setRefreshToken(response.refreshToken!)
        
    }


    const handleLogin = async () => {
        try {
            var userInfo:UserDto = {password,username};

            const response = await Client.auth_Login(userInfo);
            const { accessToken, refreshToken } = response;

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
            <button onClick={handleRegister}>Register</button>
        </div>
    );
};

export default Login;