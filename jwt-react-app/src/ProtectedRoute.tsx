import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import * as TokenUtil from "./Utils"

const ProtectedRoute: React.FC = () => {
    const token = TokenUtil.getAccessToken();
    return token ? <Outlet /> : <Navigate to="/login" />;
};

export default ProtectedRoute;