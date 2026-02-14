import { useEffect, useState } from 'react';
import { serviceRequestsApi } from '../services/api';
import type { ServiceRequest } from '../types';
import { Status } from '../types';

export default function Dashboard() {
  const [requests, setRequests] = useState<ServiceRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    try {
      setLoading(true);
      const response = await serviceRequestsApi.getAll();
      setRequests(response.data);
      setError(null);
    } catch (err) {
      setError('Failed to load service requests');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const getStatusCounts = () => {
    return {
      open: requests.filter(r => r.status === Status.Open).length,
      inProgress: requests.filter(r => r.status === Status.InProgress).length,
      complete: requests.filter(r => r.status === Status.Complete).length,
    };
  };

  const getStatusName = (status: number) => {
    switch (status) {
      case Status.Open: return 'Open';
      case Status.InProgress: return 'In Progress';
      case Status.Complete: return 'Complete';
      default: return 'Unknown';
    }
  };

  const statusCounts = getStatusCounts();

  if (loading) return <div className="loading">Loading...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="dashboard">
      <h2>Dashboard</h2>
      
      <div className="stats-grid">
        <div className="stat-card">
          <h3>Open</h3>
          <div className="stat-number">{statusCounts.open}</div>
        </div>
        <div className="stat-card">
          <h3>In Progress</h3>
          <div className="stat-number">{statusCounts.inProgress}</div>
        </div>
        <div className="stat-card">
          <h3>Complete</h3>
          <div className="stat-number">{statusCounts.complete}</div>
        </div>
        <div className="stat-card">
          <h3>Total</h3>
          <div className="stat-number">{requests.length}</div>
        </div>
      </div>

      <div className="recent-requests">
        <h3>Recent Service Requests</h3>
        <div className="requests-list">
          {requests.slice(0, 10).map(request => (
            <div key={request.id} className="request-card">
              <div className="request-header">
                <h4>{request.title}</h4>
                <span className={`status-badge status-${getStatusName(request.status).toLowerCase().replace(' ', '')}`}>
                  {getStatusName(request.status)}
                </span>
              </div>
              <div className="request-details">
                <p><strong>Property:</strong> {request.propertyAddress}</p>
                <p><strong>Client:</strong> {request.clientName}</p>
                {request.assignedTechName && (
                  <p><strong>Assigned to:</strong> {request.assignedTechName}</p>
                )}
                {request.scheduledAt && (
                  <p><strong>Scheduled:</strong> {new Date(request.scheduledAt).toLocaleDateString()}</p>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
