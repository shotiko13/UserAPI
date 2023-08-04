import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage"; 
import RegisterPage from "./pages/RegisterPage";
import UserManagementPage from "./pages/UserManagementPage";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/userManagement" element={<UserManagementPage />} /> 
      </Routes>
    </Router>
  );
}


export default App;
