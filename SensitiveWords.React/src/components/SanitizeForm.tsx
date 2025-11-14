import { useState } from 'react';
import { AlertCircle, Check, Send } from 'lucide-react';
import { sanitizeApi } from '../services/api';
import { SanitizeResponse } from '../types';

export function SanitizeForm() {
  const [message, setMessage] = useState('');
  const [result, setResult] = useState<SanitizeResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!message.trim()) return;

    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const response = await sanitizeApi.sanitize({ message });
      setResult(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to sanitize message');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
      <h2 className="text-2xl font-semibold text-slate-900 mb-6">Sanitize Message</h2>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="message" className="block text-sm font-medium text-slate-700 mb-2">
            Enter Message
          </label>
          <textarea
            id="message"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            placeholder="Type your message here... (e.g., SELECT * FROM users)"
            className="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-lime-500 focus:border-lime-500 transition-colors resize-none"
            rows={4}
            required
          />
        </div>

        <button
          type="submit"
          disabled={loading || !message.trim()}
          className="w-full flex items-center justify-center gap-2 bg-lime-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-lime-700 focus:outline-none focus:ring-2 focus:ring-lime-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Send className="w-5 h-5" />
          {loading ? 'Sanitizing...' : 'Sanitize Message'}
        </button>
      </form>

      {error && (
        <div className="mt-4 p-4 bg-red-50 border border-red-200 rounded-lg flex items-start gap-3">
          <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
          <div>
            <p className="text-sm font-medium text-red-800">Error</p>
            <p className="text-sm text-red-700 mt-1">{error}</p>
          </div>
        </div>
      )}

      {result && (
        <div className="mt-6 space-y-4">
          <div className="p-4 bg-lime-50 border border-lime-200 rounded-lg flex items-start gap-3">
            <Check className="w-5 h-5 text-lime-600 flex-shrink-0 mt-0.5" />
            <div className="flex-1">
              <p className="text-sm font-medium text-lime-800">
                Successfully sanitized {result.wordsReplaced} word{result.wordsReplaced !== 1 ? 's' : ''}
              </p>
            </div>
          </div>

          <div className="space-y-3">
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-2">
                Original Message
              </label>
              <div className="p-4 bg-slate-50 border border-slate-200 rounded-lg">
                <p className="text-sm text-slate-900 font-mono break-words">
                  {result.originalMessage}
                </p>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-700 mb-2">
                Sanitized Message
              </label>
              <div className="p-4 bg-lime-50 border border-lime-200 rounded-lg">
                <p className="text-sm text-lime-900 font-mono break-words">
                  {result.sanitizedMessage}
                </p>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
