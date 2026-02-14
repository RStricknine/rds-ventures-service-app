export enum Priority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export enum Status {
  Open = 0,
  InProgress = 1,
  Complete = 2
}

export interface Client {
  id: number;
  name: string;
  contactEmail: string;
  phone?: string;
}

export interface Property {
  id: number;
  clientId: number;
  address: string;
  city: string;
  state: string;
  zip: string;
  notes?: string;
  client?: Client;
}

export interface Technician {
  id: number;
  name: string;
  email: string;
  hourlyRate: number;
}

export interface ServiceRequest {
  id: number;
  propertyId: number;
  title: string;
  description?: string;
  priority: Priority;
  status: Status;
  scheduledAt?: string;
  completedAt?: string;
  assignedTechId?: number;
  assignedTechName?: string;
  propertyAddress?: string;
  clientName?: string;
}

export interface CreateServiceRequestDto {
  propertyId: number;
  title: string;
  description?: string;
  priority: Priority;
  scheduledAt?: string;
}

export interface UpdateServiceRequestDto {
  title?: string;
  description?: string;
  priority?: Priority;
  status?: Status;
  scheduledAt?: string;
  assignedTechId?: number;
}

export interface TimeEntry {
  id: number;
  techId: number;
  serviceRequestId: number;
  startUtc: string;
  endUtc?: string;
  durationMinutes?: number;
  weekStartMondayUtc: string;
  technicianName?: string;
  serviceRequestTitle?: string;
}

export interface StartTimeEntryDto {
  serviceRequestId: number;
  techId: number;
}

export interface StopTimeEntryDto {
  timeEntryId: number;
}

export interface WeeklyTime {
  techId: number;
  technicianName: string;
  weekStart: string;
  totalMinutes: number;
  totalHours: number;
}

export interface Attachment {
  id: number;
  serviceRequestId: number;
  techId: number;
  blobUrl: string;
  contentType: string;
  uploadedUtc: string;
  caption?: string;
  technicianName?: string;
}

export interface CreateAttachmentDto {
  serviceRequestId: number;
  techId: number;
  fileName: string;
  contentType: string;
  caption?: string;
}

export interface SasTokenResponse {
  sasUrl: string;
  blobUrl: string;
  expiresOn: string;
}
