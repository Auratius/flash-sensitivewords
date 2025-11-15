import {
  SensitiveWord,
  CreateSensitiveWordRequest,
  UpdateSensitiveWordRequest,
  SanitizeRequest,
  SanitizeResponse,
  HealthStats,
  OperationStat,
} from '../types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7001/api';
const METRICS_BASE_URL = API_BASE_URL.replace('/api', '');

export const sensitiveWordsApi = {
  async getAll(): Promise<SensitiveWord[]> {
    const response = await fetch(`${API_BASE_URL}/sensitivewords`);
    if (!response.ok) throw new Error('Failed to fetch sensitive words');
    return response.json();
  },

  async getById(id: string): Promise<SensitiveWord> {
    const response = await fetch(`${API_BASE_URL}/sensitivewords/${id}`);
    if (!response.ok) throw new Error('Failed to fetch sensitive word');
    return response.json();
  },

  async create(request: CreateSensitiveWordRequest): Promise<{ id: string }> {
    const response = await fetch(`${API_BASE_URL}/sensitivewords`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Failed to create sensitive word');
    }
    return response.json();
  },

  async update(id: string, request: UpdateSensitiveWordRequest): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/sensitivewords/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'Failed to update sensitive word');
    }
  },

  async delete(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/sensitivewords/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) throw new Error('Failed to delete sensitive word');
  },
};

export const sanitizeApi = {
  async sanitize(request: SanitizeRequest): Promise<SanitizeResponse> {
    const response = await fetch(`${API_BASE_URL}/sanitize`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });
    if (!response.ok) throw new Error('Failed to sanitize message');
    return response.json();
  },
};

export const healthApi = {
  async getHealth(): Promise<HealthStats> {
    const response = await fetch(`${METRICS_BASE_URL}/metrics`);
    if (!response.ok) throw new Error('Failed to fetch health stats');
    return response.json();
  },
};

export const statisticsApi = {
  async getAll(): Promise<OperationStat[]> {
    const response = await fetch(`${API_BASE_URL}/statistics`);
    if (!response.ok) throw new Error('Failed to fetch operation statistics');
    return response.json();
  },

  async getByType(operationType: string): Promise<OperationStat[]> {
    const response = await fetch(`${API_BASE_URL}/statistics/${operationType}`);
    if (!response.ok) throw new Error(`Failed to fetch statistics for ${operationType}`);
    return response.json();
  },

  async reset(): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/statistics/reset`, {
      method: 'POST',
    });
    if (!response.ok) throw new Error('Failed to reset statistics');
  },
};
