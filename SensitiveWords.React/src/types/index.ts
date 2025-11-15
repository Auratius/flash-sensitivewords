export interface SensitiveWord {
  id: string;
  word: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateSensitiveWordRequest {
  word: string;
}

export interface UpdateSensitiveWordRequest {
  word: string;
  isActive: boolean;
}

export interface SanitizeRequest {
  message: string;
}

export interface SanitizeResponse {
  originalMessage: string;
  sanitizedMessage: string;
  wordsReplaced: number;
}

export interface HealthStats {
  timestamp: string;
  uptime: string;
  memoryUsageMB: number;
  cpuTimeSeconds: number;
  threadCount: number;
}

export interface OperationStat {
  id: number;
  operationType: string;
  resourceType: string;
  count: number;
  lastUpdated: string;
}
