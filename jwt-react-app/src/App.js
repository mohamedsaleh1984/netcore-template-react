import React, { useState } from "react";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import Login from "./components/Login";
import ProtectedData from "./components/ProtectedData";

const App = () => {
  const [token, setToken] = useState("");

  return (
    <Router>
      <div>
        <nav style={{ marginBottom: "20px" }}>
          <Link to="/" style={{ marginRight: "10px" }}>Login</Link>
          <Link to="/protected">Protected Data</Link>
        </nav>
        <Routes>
          <Route path="/" element={<Login setToken={setToken} />} />
          <Route path="/protected" element={<ProtectedData token={token} />} />
        </Routes>
      </div>
    </Router>
  );
};

export default App;
