import React, { useState, useEffect } from "react";
import axios from "axios";

const ProtectedData = ({ token }) => {
  const [data, setData] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await axios.get("http://localhost:5000/api/weatherforecast", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        setData(response.data);
      } catch (err) {
        setError("You are not authorized to view this data.");
      }
    };

    fetchData();
  }, [token]);

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
