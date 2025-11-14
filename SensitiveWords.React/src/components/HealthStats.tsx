import { useEffect, useState } from 'react';
import { Activity, Clock, Cpu, HardDrive, Layers } from 'lucide-react';
import { healthApi } from '../services/api';
import { HealthStats as HealthStatsType } from '../types';

export function HealthStats() {
  const [stats, setStats] = useState<HealthStatsType | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchHealth = async () => {
    try {
      const data = await healthApi.getHealth();
      setStats(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch health stats');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchHealth();
    const interval = setInterval(fetchHealth, 5000);
    return () => clearInterval(interval);
  }, []);

  if (loading && !stats) {
    return (
      <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-6">
        <div className="flex items-center justify-center">
          <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-lime-600"></div>
        </div>
      </div>
    );
  }

  if (error && !stats) {
    return (
      <div className="bg-red-50 rounded-lg border border-red-200 p-4">
        <p className="text-sm text-red-700 text-center">{error}</p>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
      <div className="flex items-center justify-between mb-3">
        <h2 className="text-sm font-semibold text-slate-900 flex items-center gap-2">
          <Activity className="w-4 h-4 text-lime-600" />
          System Metrics
        </h2>
        <div className="flex items-center gap-2">
          <div className="w-1.5 h-1.5 rounded-full bg-lime-500 animate-pulse"></div>
          <span className="text-xs text-slate-600">Online</span>
          {stats?.timestamp && (
            <span className="text-xs text-slate-500">
              · {new Date(stats.timestamp).toLocaleTimeString()}
            </span>
          )}
        </div>
      </div>

      <div className="grid grid-cols-4 gap-3">
        <div className="flex items-center gap-2 p-2 bg-lime-50 rounded-lg">
          <Clock className="w-6 h-6 text-lime-600 flex-shrink-0" />
          <div>
            <p className="text-sm font-bold text-slate-900">{stats?.uptime || '00:00:00'}</p>
            <p className="text-xs text-slate-600">Uptime</p>
          </div>
        </div>

        <div className="flex items-center gap-2 p-2 bg-lime-50 rounded-lg">
          <HardDrive className="w-6 h-6 text-lime-600 flex-shrink-0" />
          <div>
            <p className="text-sm font-bold text-slate-900">{stats?.memoryUsageMB || 0} MB</p>
            <p className="text-xs text-slate-600">Memory</p>
          </div>
        </div>

        <div className="flex items-center gap-2 p-2 bg-lime-50 rounded-lg">
          <Cpu className="w-6 h-6 text-lime-600 flex-shrink-0" />
          <div>
            <p className="text-sm font-bold text-slate-900">{stats?.cpuTimeSeconds.toFixed(2) || 0}s</p>
            <p className="text-xs text-slate-600">CPU Time</p>
          </div>
        </div>

        <div className="flex items-center gap-2 p-2 bg-lime-50 rounded-lg">
          <Layers className="w-6 h-6 text-lime-600 flex-shrink-0" />
          <div>
            <p className="text-sm font-bold text-slate-900">{stats?.threadCount || 0}</p>
            <p className="text-xs text-slate-600">Threads</p>
          </div>
        </div>
      </div>
    </div>
  );
}
