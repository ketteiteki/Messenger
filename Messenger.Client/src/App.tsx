import React from "react";
import { Routes, Route, BrowserRouter } from "react-router-dom";
import { Login } from "./pages/Login";
import { Registration } from "./pages/Registration";
import "./App.scss";
import { Layout } from "./pages/Layout";

const App = () => {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/registration" element={<Registration />} />
          <Route path="/" element={<Layout />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
};

export default App;
