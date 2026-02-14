import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import ServiceRequestsPage from './pages/ServiceRequestsPage';
import TimeTrackingPage from './pages/TimeTrackingPage';
import './App.css';

function App() {
  return (
    <Router>
      <div className="app">
        <header className="app-header">
          <div className="header-content">
            <h1>RDS Ventures LLC</h1>
            <nav className="main-nav">
              <Link to="/">Dashboard</Link>
              <Link to="/service-requests">Service Requests</Link>
              <Link to="/time-tracking">Time Tracking</Link>
            </nav>
          </div>
        </header>
        <main className="app-main">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/service-requests" element={<ServiceRequestsPage />} />
            <Route path="/time-tracking" element={<TimeTrackingPage />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
