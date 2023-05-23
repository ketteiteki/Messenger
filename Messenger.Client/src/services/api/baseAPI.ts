import { authorizationState } from "../../state/AuthorizationState";
import axios from "axios";
import TokenService from "../messenger/TokenService";

const api = axios.create({
  baseURL: `${process.env.REACT_APP_BASE_API}/api`,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("AuthorizationToken");
    if (token) {
      config.headers = {
        ...config.headers,
        Authorization: `Bearer ${token}`,
      };
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (res) => {
    return res;
  },
  async (err) => {
    const originalConfig = err.config;

    if (
      originalConfig.url !== "/Auth/login" &&
      originalConfig.url !== "/Auth/registration" &&
      err.response
    ) {
      if (err.response.status === 401 && !originalConfig._retry) {
        originalConfig._retry = true;
        console.log(3);
        authorizationState.incrementCountFailRefresh();
        try {
          const refreshToken = TokenService.getLocalRefreshToken();

          const response = await api.post(`/Auth/refresh/${refreshToken}`);

          const { accessToken } = response.data;
          TokenService.setLocalAccessToken(accessToken);

          return api(originalConfig);
        } catch (_error) {
          return Promise.reject(_error);
        }
      }
    }
    return Promise.reject(err);
  }
);

export default api;
