import React, { useState, useEffect } from "react";

import { Client } from '../api/Client';

const ProtectedData: React.FC = ({ token }) => {
  const [data, setData] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    async () => {
      const response = await Client.weatherForecast_Get();
      setError("You are not authorized to view this data.");
    }
  }, [])


  return (
    <div style={{ padding: "20px" }}>
      <h2>Protected Data</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {data && (
        <pre>
          <code>{JSON.stringify(data, null, 2)}</code>
        </pre>
      )}
    </div>
  );
};

export default ProtectedData;
