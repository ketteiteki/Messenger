import React from "react";
import "./App.scss";
import { Routes, Route, BrowserRouter } from "react-router-dom";
import { observer } from "mobx-react-lite";
import Layout from "./pages/layout/Layout";
import Login from "./pages/login/Login";
import Registration from "./pages/registration/Registration";
import ChatInfo from "./components/chatInfo/ChatInfo";
import ProfileInfo from "./components/profileInfo/ProfileInfo";
import CreateChat from "./components/createChat/CreateChat";

const App = observer(() => {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route path="" element={<ProfileInfo />} />
            <Route path="chatInfo" element={<ChatInfo />} />
            <Route path="createChat" element={<CreateChat />} />
          </Route>
          <Route path="/login" element={<Login />} />
          <Route path="/registration" element={<Registration />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
});

export default App;
