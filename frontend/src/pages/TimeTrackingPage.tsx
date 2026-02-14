import { useEffect, useState } from 'react';
import { timeEntriesApi, serviceRequestsApi, techniciansApi } from '../services/api';
import type { TimeEntry, ServiceRequest, Technician, WeeklyTime } from '../types';

export default function TimeTrackingPage() {
  const [timeEntries, setTimeEntries] = useState<TimeEntry[]>([]);
  const [weeklyTotals, setWeeklyTotals] = useState<WeeklyTime[]>([]);
  const [serviceRequests, setServiceRequests] = useState<ServiceRequest[]>([]);
  const [technicians, setTechnicians] = useState<Technician[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedTechId, setSelectedTechId] = useState<number | null>(null);
  const [selectedRequestId, setSelectedRequestId] = useState<number | null>(null);
  const [activeEntry, setActiveEntry] = useState<TimeEntry | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [entriesRes, weeklyRes, requestsRes, techsRes] = await Promise.all([
        timeEntriesApi.getAll(),
        timeEntriesApi.getWeekly(),
        serviceRequestsApi.getAll(),
        techniciansApi.getAll(),
      ]);
      setTimeEntries(entriesRes.data);
      setWeeklyTotals(weeklyRes.data);
      setServiceRequests(requestsRes.data);
      setTechnicians(techsRes.data);
      
      // Find active entry (one with no end time)
      const active = entriesRes.data.find(e => !e.endUtc);
      setActiveEntry(active || null);
      
      setError(null);
    } catch (err) {
      setError('Failed to load data');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleStartTime = async () => {
    if (!selectedTechId || !selectedRequestId) {
      alert('Please select a technician and service request');
      return;
    }

    try {
      await timeEntriesApi.start({ techId: selectedTechId, serviceRequestId: selectedRequestId });
      await loadData();
    } catch (err) {
      console.error('Failed to start time entry', err);
    }
  };

  const handleStopTime = async () => {
    if (!activeEntry) return;

    try {
      await timeEntriesApi.stop({ timeEntryId: activeEntry.id });
      await loadData();
    } catch (err) {
      console.error('Failed to stop time entry', err);
    }
  };

  const formatDuration = (minutes?: number) => {
    if (!minutes) return 'In progress...';
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h ${mins}m`;
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="time-tracking-page">
      <h2>Time Tracking</h2>

      <div className="time-entry-form">
        <h3>Log Time</h3>
        <div className="form-row">
          <div className="form-group">
            <label>Technician:</label>
            <select
              value={selectedTechId || ''}
              onChange={(e) => setSelectedTechId(Number(e.target.value))}
              disabled={!!activeEntry}
            >
              <option value="">Select Technician</option>
              {technicians.map(tech => (
                <option key={tech.id} value={tech.id}>{tech.name}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Service Request:</label>
            <select
              value={selectedRequestId || ''}
              onChange={(e) => setSelectedRequestId(Number(e.target.value))}
              disabled={!!activeEntry}
            >
              <option value="">Select Service Request</option>
              {serviceRequests.map(request => (
                <option key={request.id} value={request.id}>{request.title}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            {activeEntry ? (
              <button onClick={handleStopTime} className="btn btn-danger">
                Stop Time
              </button>
            ) : (
              <button onClick={handleStartTime} className="btn btn-primary">
                Start Time
              </button>
            )}
          </div>
        </div>
        {activeEntry && (
          <div className="active-entry">
            <p><strong>Active Entry:</strong> {activeEntry.serviceRequestTitle}</p>
            <p><strong>Started:</strong> {new Date(activeEntry.startUtc).toLocaleTimeString()}</p>
          </div>
        )}
      </div>

      <div className="weekly-totals">
        <h3>Weekly Totals</h3>
        <table>
          <thead>
            <tr>
              <th>Technician</th>
              <th>Week Start</th>
              <th>Total Hours</th>
            </tr>
          </thead>
          <tbody>
            {weeklyTotals.map((total, index) => (
              <tr key={index}>
                <td>{total.technicianName}</td>
                <td>{new Date(total.weekStart).toLocaleDateString()}</td>
                <td>{total.totalHours.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="time-entries">
        <h3>Time Entries</h3>
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Technician</th>
              <th>Service Request</th>
              <th>Start</th>
              <th>End</th>
              <th>Duration</th>
            </tr>
          </thead>
          <tbody>
            {timeEntries.map(entry => (
              <tr key={entry.id}>
                <td>{entry.id}</td>
                <td>{entry.technicianName}</td>
                <td>{entry.serviceRequestTitle}</td>
                <td>{new Date(entry.startUtc).toLocaleString()}</td>
                <td>{entry.endUtc ? new Date(entry.endUtc).toLocaleString() : 'In progress'}</td>
                <td>{formatDuration(entry.durationMinutes)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
