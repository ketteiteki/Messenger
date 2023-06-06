import axios from "axios";

const api = axios.create({
  baseURL: `${process.env.REACT_APP_BASE_API}/api`,
  headers: {
    "Content-Type": "application/json",
    "Accept": "*/*"
  },
  withCredentials: true
});

export default api;
