import axios from 'axios';
import type {
  ServiceRequest,
  CreateServiceRequestDto,
  UpdateServiceRequestDto,
  TimeEntry,
  StartTimeEntryDto,
  StopTimeEntryDto,
  WeeklyTime,
  Attachment,
  CreateAttachmentDto,
  SasTokenResponse,
  Client,
  Property,
  Technician
} from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Service Requests
export const serviceRequestsApi = {
  getAll: () => api.get<ServiceRequest[]>('/servicerequests'),
  getById: (id: number) => api.get<ServiceRequest>(`/servicerequests/${id}`),
  create: (data: CreateServiceRequestDto) => api.post<ServiceRequest>('/servicerequests', data),
  update: (id: number, data: UpdateServiceRequestDto) => api.put(`/servicerequests/${id}`, data),
  delete: (id: number) => api.delete(`/servicerequests/${id}`),
  assign: (id: number, technicianId: number) => api.post(`/servicerequests/${id}/assign`, technicianId),
};

// Time Entries
export const timeEntriesApi = {
  getAll: () => api.get<TimeEntry[]>('/timeentries'),
  getByTech: (techId: number) => api.get<TimeEntry[]>(`/timeentries/technician/${techId}`),
  start: (data: StartTimeEntryDto) => api.post<TimeEntry>('/timeentries/start', data),
  stop: (data: StopTimeEntryDto) => api.post<TimeEntry>('/timeentries/stop', data),
  getWeekly: (weekStart?: string) => api.get<WeeklyTime[]>('/timeentries/weekly', { params: { weekStart } }),
};

// Attachments
export const attachmentsApi = {
  getAll: () => api.get<Attachment[]>('/attachments'),
  getByServiceRequest: (serviceRequestId: number) => api.get<Attachment[]>(`/attachments/service-request/${serviceRequestId}`),
  getSasToken: (data: CreateAttachmentDto) => api.post<SasTokenResponse>('/attachments/sas-token', data),
  delete: (id: number) => api.delete(`/attachments/${id}`),
};

// Clients
export const clientsApi = {
  getAll: () => api.get<Client[]>('/clients'),
  getById: (id: number) => api.get<Client>(`/clients/${id}`),
  create: (data: Omit<Client, 'id'>) => api.post<Client>('/clients', data),
  update: (id: number, data: Client) => api.put(`/clients/${id}`, data),
  delete: (id: number) => api.delete(`/clients/${id}`),
};

// Properties
export const propertiesApi = {
  getAll: () => api.get<Property[]>('/properties'),
  getById: (id: number) => api.get<Property>(`/properties/${id}`),
  create: (data: Omit<Property, 'id'>) => api.post<Property>('/properties', data),
  update: (id: number, data: Property) => api.put(`/properties/${id}`, data),
  delete: (id: number) => api.delete(`/properties/${id}`),
};

// Technicians
export const techniciansApi = {
  getAll: () => api.get<Technician[]>('/technicians'),
  getById: (id: number) => api.get<Technician>(`/technicians/${id}`),
  create: (data: Omit<Technician, 'id'>) => api.post<Technician>('/technicians', data),
  update: (id: number, data: Technician) => api.put(`/technicians/${id}`, data),
  delete: (id: number) => api.delete(`/technicians/${id}`),
};

export default api;
