import { useEffect, useState } from 'react';
import { Activity, TrendingUp, RefreshCw, RotateCcw, AlertCircle } from 'lucide-react';
import { statisticsApi } from '../services/api';
import { OperationStat } from '../types';

export function OperationStats() {
  const [stats, setStats] = useState<OperationStat[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [lastRefresh, setLastRefresh] = useState<Date>(new Date());
  const [resetting, setResetting] = useState(false);

  const fetchStats = async () => {
    try {
      setLoading(true);
      const data = await statisticsApi.getAll();
      setStats(data);
      setLastRefresh(new Date());
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch statistics');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStats();

    // Auto-refresh every 10 seconds
    const interval = setInterval(fetchStats, 10000);
    return () => clearInterval(interval);
  }, []);

  const handleReset = async () => {
    if (!confirm('Are you sure you want to reset all operation statistics to zero? This action cannot be undone.')) {
      return;
    }

    try {
      setResetting(true);
      await statisticsApi.reset();
      await fetchStats();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to reset statistics');
    } finally {
      setResetting(false);
    }
  };

  const getOperationIcon = (type: string) => {
    switch (type.toUpperCase()) {
      case 'CREATE':
        return '➕';
      case 'READ':
        return '📖';
      case 'UPDATE':
        return '✏️';
      case 'DELETE':
        return '🗑️';
      case 'SANITIZE':
        return '🧹';
      default:
        return '📊';
    }
  };

  const getOperationColor = (type: string) => {
    switch (type.toUpperCase()) {
      case 'CREATE':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'READ':
        return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'UPDATE':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'DELETE':
        return 'bg-red-100 text-red-800 border-red-200';
      case 'SANITIZE':
        return 'bg-purple-100 text-purple-800 border-purple-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const totalOperations = stats.reduce((sum, stat) => sum + stat.count, 0);

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <Activity className="w-6 h-6 text-lime-600" />
          <h2 className="text-2xl font-semibold text-slate-900">API Operation Statistics</h2>
        </div>
        <div className="flex items-center gap-2">
          <button
            onClick={fetchStats}
            disabled={loading}
            className="flex items-center gap-2 px-3 py-2 text-sm text-slate-600 bg-slate-100 rounded-lg hover:bg-slate-200 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            title="Refresh statistics"
          >
            <RefreshCw className={`w-4 h-4 ${loading ? 'animate-spin' : ''}`} />
            Refresh
          </button>
          <button
            onClick={handleReset}
            disabled={resetting || loading}
            className="flex items-center gap-2 px-3 py-2 text-sm text-white bg-red-600 rounded-lg hover:bg-red-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            title="Reset all statistics to zero"
          >
            <RotateCcw className={`w-4 h-4 ${resetting ? 'animate-spin' : ''}`} />
            Reset
          </button>
        </div>
      </div>

      {error && (
        <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg flex items-start gap-3">
          <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
          <div>
            <p className="text-sm font-medium text-red-800">Error</p>
            <p className="text-sm text-red-700 mt-1">{error}</p>
          </div>
        </div>
      )}

      {loading && stats.length === 0 ? (
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-lime-600"></div>
        </div>
      ) : (
        <>
          {/* Total Operations Card */}
          <div className="mb-6 p-6 bg-gradient-to-r from-lime-600 to-lime-700 rounded-lg text-white">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-lime-100 text-sm font-medium mb-1">Total Operations</p>
                <p className="text-4xl font-bold">{totalOperations.toLocaleString()}</p>
              </div>
              <TrendingUp className="w-12 h-12 text-lime-200" />
            </div>
          </div>

          {/* Operation Type Cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {stats.map((stat) => (
              <div
                key={stat.id}
                className={`p-4 border rounded-lg ${getOperationColor(stat.operationType)}`}
              >
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center gap-2">
                    <span className="text-2xl">{getOperationIcon(stat.operationType)}</span>
                    <div>
                      <p className="font-semibold text-lg">{stat.operationType}</p>
                      <p className="text-xs opacity-75">{stat.resourceType}</p>
                    </div>
                  </div>
                </div>
                <div className="mb-2">
                  <p className="text-3xl font-bold">{stat.count.toLocaleString()}</p>
                  <p className="text-xs opacity-75 mt-1">operations</p>
                </div>
                <div className="text-xs opacity-60">
                  Last updated: {new Date(stat.lastUpdated).toLocaleTimeString()}
                </div>
              </div>
            ))}
          </div>

          {/* Last Refresh Time */}
          <div className="mt-4 text-center text-xs text-slate-500">
            Last refreshed: {lastRefresh.toLocaleTimeString()} • Auto-refreshes every 10 seconds
          </div>
        </>
      )}
    </div>
  );
}
