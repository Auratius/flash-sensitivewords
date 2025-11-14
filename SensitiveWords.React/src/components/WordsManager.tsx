import { useEffect, useState } from 'react';
import { Plus, Trash2, Edit2, X, Check, AlertCircle, Search } from 'lucide-react';
import { sensitiveWordsApi } from '../services/api';
import { SensitiveWord } from '../types';

export function WordsManager() {
  const [words, setWords] = useState<SensitiveWord[]>([]);
  const [filteredWords, setFilteredWords] = useState<SensitiveWord[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [newWord, setNewWord] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editWord, setEditWord] = useState('');
  const [editActive, setEditActive] = useState(true);

  const fetchWords = async () => {
    try {
      setLoading(true);
      const data = await sensitiveWordsApi.getAll();
      setWords(data);
      setFilteredWords(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch words');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchWords();
  }, []);

  useEffect(() => {
    const filtered = words.filter(word =>
      word.word.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredWords(filtered);
  }, [searchTerm, words]);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newWord.trim()) return;

    try {
      await sensitiveWordsApi.create({ word: newWord.trim() });
      setNewWord('');
      await fetchWords();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create word');
    }
  };

  const handleEdit = (word: SensitiveWord) => {
    setEditingId(word.id);
    setEditWord(word.word);
    setEditActive(word.isActive);
  };

  const handleUpdate = async (id: string) => {
    try {
      await sensitiveWordsApi.update(id, { word: editWord.trim(), isActive: editActive });
      setEditingId(null);
      await fetchWords();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update word');
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this word?')) return;

    try {
      await sensitiveWordsApi.delete(id);
      await fetchWords();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete word');
    }
  };

  return (
    <div className="h-full flex flex-col">
      <h2 className="text-2xl font-semibold text-slate-900 mb-6">Manage Sensitive Words</h2>

      <form onSubmit={handleCreate} className="mb-4">
        <div className="flex gap-2">
          <input
            type="text"
            value={newWord}
            onChange={(e) => setNewWord(e.target.value)}
            placeholder="Add new sensitive word..."
            className="flex-1 px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-lime-500 focus:border-lime-500 transition-colors"
          />
          <button
            type="submit"
            disabled={!newWord.trim()}
            className="flex items-center gap-2 bg-lime-600 text-white px-4 py-2 rounded-lg font-medium hover:bg-lime-700 focus:outline-none focus:ring-2 focus:ring-lime-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            <Plus className="w-5 h-5" />
            Add Word
          </button>
        </div>
      </form>

      <div className="mb-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Search words..."
            className="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-lime-500 focus:border-lime-500 transition-colors"
          />
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

      {loading ? (
        <div className="flex items-center justify-center" style={{ height: '420px' }}>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-lime-600"></div>
        </div>
      ) : (
        <div className="space-y-2 overflow-y-auto mb-4" style={{ height: '420px' }}>
          {filteredWords.length === 0 ? (
            <p className="text-center text-slate-500 py-8">
              {searchTerm ? 'No words found matching your search' : 'No sensitive words added yet'}
            </p>
          ) : (
            filteredWords.map((word) => (
              <div
                key={word.id}
                className="flex items-center gap-3 p-3 border border-slate-200 rounded-lg hover:bg-slate-50 transition-colors"
              >
                {editingId === word.id ? (
                  <>
                    <input
                      type="text"
                      value={editWord}
                      onChange={(e) => setEditWord(e.target.value)}
                      className="flex-1 px-3 py-1 border border-slate-300 rounded focus:ring-2 focus:ring-lime-500 focus:border-lime-500"
                    />
                    <label className="flex items-center gap-2">
                      <input
                        type="checkbox"
                        checked={editActive}
                        onChange={(e) => setEditActive(e.target.checked)}
                        className="rounded border-slate-300 text-lime-600 focus:ring-lime-500"
                      />
                      <span className="text-sm text-slate-700">Active</span>
                    </label>
                    <button
                      onClick={() => handleUpdate(word.id)}
                      className="p-2 text-lime-600 hover:bg-lime-50 rounded transition-colors"
                    >
                      <Check className="w-5 h-5" />
                    </button>
                    <button
                      onClick={() => setEditingId(null)}
                      className="p-2 text-slate-600 hover:bg-slate-100 rounded transition-colors"
                    >
                      <X className="w-5 h-5" />
                    </button>
                  </>
                ) : (
                  <>
                    <span className="flex-1 font-mono text-slate-900">{word.word}</span>
                    <span
                      className={`px-2 py-1 rounded text-xs font-medium ${
                        word.isActive
                          ? 'bg-lime-100 text-lime-800'
                          : 'bg-slate-100 text-slate-600'
                      }`}
                    >
                      {word.isActive ? 'Active' : 'Inactive'}
                    </span>
                    <button
                      onClick={() => handleEdit(word)}
                      className="p-2 text-lime-600 hover:bg-lime-50 rounded transition-colors"
                    >
                      <Edit2 className="w-5 h-5" />
                    </button>
                    <button
                      onClick={() => handleDelete(word.id)}
                      className="p-2 text-red-600 hover:bg-red-50 rounded transition-colors"
                    >
                      <Trash2 className="w-5 h-5" />
                    </button>
                  </>
                )}
              </div>
            ))
          )}
        </div>
      )}

      <div className="pt-4 border-t border-slate-200 mt-auto">
        <p className="text-sm text-slate-600">
          Total words: <span className="font-semibold">{words.length}</span> | Active:{' '}
          <span className="font-semibold text-lime-600">
            {words.filter((w) => w.isActive).length}
          </span>
        </p>
      </div>
    </div>
  );
}
