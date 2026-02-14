import { useEffect, useState } from 'react';
import { serviceRequestsApi, propertiesApi, techniciansApi } from '../services/api';
import type { ServiceRequest, Property, Technician, UpdateServiceRequestDto } from '../types';
import { Status, Priority } from '../types';

export default function ServiceRequestsPage() {
  const [requests, setRequests] = useState<ServiceRequest[]>([]);
  const [properties, setProperties] = useState<Property[]>([]);
  const [technicians, setTechnicians] = useState<Technician[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedRequest, setSelectedRequest] = useState<ServiceRequest | null>(null);
  const [isEditing, setIsEditing] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [requestsRes, propertiesRes, techniciansRes] = await Promise.all([
        serviceRequestsApi.getAll(),
        propertiesApi.getAll(),
        techniciansApi.getAll(),
      ]);
      setRequests(requestsRes.data);
      setProperties(propertiesRes.data);
      setTechnicians(techniciansRes.data);
      setError(null);
    } catch (err) {
      setError('Failed to load data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateStatus = async (id: number, status: Status) => {
    try {
      const updateData: UpdateServiceRequestDto = { status };
      await serviceRequestsApi.update(id, updateData);
      await loadData();
    } catch (err) {
      console.error('Failed to update status', err);
    }
  };

  const handleAssignTech = async (id: number, techId: number) => {
    try {
      const updateData: UpdateServiceRequestDto = { assignedTechId: techId };
      await serviceRequestsApi.update(id, updateData);
      await loadData();
    } catch (err) {
      console.error('Failed to assign technician', err);
    }
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="service-requests-page">
      <div className="page-header">
        <h2>Service Requests</h2>
      </div>

      <div className="requests-table">
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Title</th>
              <th>Property</th>
              <th>Client</th>
              <th>Priority</th>
              <th>Status</th>
              <th>Assigned To</th>
              <th>Scheduled</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {requests.map(request => (
              <tr key={request.id}>
                <td>{request.id}</td>
                <td>{request.title}</td>
                <td>{request.propertyAddress}</td>
                <td>{request.clientName}</td>
                <td>
                  <span className={`priority-badge priority-${Priority[request.priority].toLowerCase()}`}>
                    {Priority[request.priority]}
                  </span>
                </td>
                <td>
                  <select
                    value={request.status}
                    onChange={(e) => handleUpdateStatus(request.id, Number(e.target.value))}
                    className={`status-select status-${Status[request.status].toLowerCase()}`}
                  >
                    <option value={Status.Open}>Open</option>
                    <option value={Status.InProgress}>In Progress</option>
                    <option value={Status.Complete}>Complete</option>
                  </select>
                </td>
                <td>
                  <select
                    value={request.assignedTechId || ''}
                    onChange={(e) => handleAssignTech(request.id, Number(e.target.value))}
                    className="tech-select"
                  >
                    <option value="">Unassigned</option>
                    {technicians.map(tech => (
                      <option key={tech.id} value={tech.id}>{tech.name}</option>
                    ))}
                  </select>
                </td>
                <td>
                  {request.scheduledAt
                    ? new Date(request.scheduledAt).toLocaleDateString()
                    : 'Not scheduled'}
                </td>
                <td>
                  <button
                    onClick={() => setSelectedRequest(request)}
                    className="btn btn-sm"
                  >
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedRequest && (
        <div className="modal-overlay" onClick={() => setSelectedRequest(null)}>
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{selectedRequest.title}</h3>
              <button onClick={() => setSelectedRequest(null)} className="close-btn">×</button>
            </div>
            <div className="modal-body">
              <p><strong>Description:</strong> {selectedRequest.description || 'No description'}</p>
              <p><strong>Property:</strong> {selectedRequest.propertyAddress}</p>
              <p><strong>Client:</strong> {selectedRequest.clientName}</p>
              <p><strong>Priority:</strong> {Priority[selectedRequest.priority]}</p>
              <p><strong>Status:</strong> {Status[selectedRequest.status]}</p>
              {selectedRequest.assignedTechName && (
                <p><strong>Assigned to:</strong> {selectedRequest.assignedTechName}</p>
              )}
              {selectedRequest.scheduledAt && (
                <p><strong>Scheduled:</strong> {new Date(selectedRequest.scheduledAt).toLocaleString()}</p>
              )}
              {selectedRequest.completedAt && (
                <p><strong>Completed:</strong> {new Date(selectedRequest.completedAt).toLocaleString()}</p>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
