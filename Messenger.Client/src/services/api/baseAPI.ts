import axios from "axios";

const api = axios.create({
  baseURL: `${process.env.REACT_APP_BASE_API}/api`,
  headers: {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "*"
  },
  withCredentials: true
});

export default api;
