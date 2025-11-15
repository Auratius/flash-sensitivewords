import { useEffect, useState } from 'react';
import { Plus, Trash2, Edit2, AlertCircle, Search } from 'lucide-react';
import { sensitiveWordsApi } from '../services/api';
import { SensitiveWord } from '../types';
import { EditWordModal } from './EditWordModal';
import { DeleteConfirmModal } from './DeleteConfirmModal';

export function WordsManager() {
  const [words, setWords] = useState<SensitiveWord[]>([]);
  const [filteredWords, setFilteredWords] = useState<SensitiveWord[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Modal states
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [editingWord, setEditingWord] = useState<SensitiveWord | null>(null);
  const [deletingWord, setDeletingWord] = useState<SensitiveWord | null>(null);
  const [modalMode, setModalMode] = useState<'add' | 'edit'>('add');

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

  const openAddModal = () => {
    setModalMode('add');
    setEditingWord(null);
    setIsEditModalOpen(true);
  };

  const openEditModal = (word: SensitiveWord) => {
    setModalMode('edit');
    setEditingWord(word);
    setIsEditModalOpen(true);
  };

  const openDeleteModal = (word: SensitiveWord) => {
    setDeletingWord(word);
    setIsDeleteModalOpen(true);
  };

  const handleSaveWord = async (word: string, isActive: boolean) => {
    try {
      if (modalMode === 'add') {
        await sensitiveWordsApi.create({ word });
      } else if (editingWord) {
        await sensitiveWordsApi.update(editingWord.id, { word, isActive });
      }
      await fetchWords();
      setError(null);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to save word';
      setError(errorMessage);
      throw new Error(errorMessage);
    }
  };

  const handleConfirmDelete = async () => {
    if (!deletingWord) return;

    try {
      await sensitiveWordsApi.delete(deletingWord.id);
      await fetchWords();
      setError(null);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to delete word';
      setError(errorMessage);
      throw new Error(errorMessage);
    }
  };

  return (
    <div className="h-full flex flex-col">
      <h2 className="text-2xl font-semibold text-slate-900 mb-6">Manage Sensitive Words</h2>

      <div className="mb-4">
        <button
          onClick={openAddModal}
          className="flex items-center gap-2 bg-lime-600 text-white px-4 py-2 rounded-lg font-medium hover:bg-lime-700 focus:outline-none focus:ring-2 focus:ring-lime-500 focus:ring-offset-2 transition-colors"
        >
          <Plus className="w-5 h-5" />
          Add New Word
        </button>
      </div>

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
        <div className="overflow-y-auto mb-4" style={{ height: '420px' }}>
          {filteredWords.length === 0 ? (
            <p className="text-center text-slate-500 py-8">
              {searchTerm ? 'No words found matching your search' : 'No sensitive words added yet'}
            </p>
          ) : (
            <div className="grid grid-cols-3 gap-3">
              {filteredWords.map((word) => (
                <div
                  key={word.id}
                  className="flex flex-col gap-2 p-3 border border-slate-200 rounded-lg hover:bg-slate-50 transition-colors"
                >
                  <div className="flex items-start justify-between gap-2">
                    <span className="flex-1 font-mono text-slate-900 break-words">{word.word}</span>
                    <span
                      className={`px-2 py-1 rounded text-xs font-medium whitespace-nowrap ${
                        word.isActive
                          ? 'bg-lime-100 text-lime-800'
                          : 'bg-slate-100 text-slate-600'
                      }`}
                    >
                      {word.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </div>
                  <div className="flex gap-2">
                    <button
                      onClick={() => openEditModal(word)}
                      className="flex-1 flex items-center justify-center gap-1 px-2 py-1.5 text-sm text-lime-600 bg-lime-50 hover:bg-lime-100 rounded transition-colors"
                      title="Edit word"
                    >
                      <Edit2 className="w-4 h-4" />
                      Edit
                    </button>
                    <button
                      onClick={() => openDeleteModal(word)}
                      className="flex-1 flex items-center justify-center gap-1 px-2 py-1.5 text-sm text-red-600 bg-red-50 hover:bg-red-100 rounded transition-colors"
                      title="Delete word"
                    >
                      <Trash2 className="w-4 h-4" />
                      Delete
                    </button>
                  </div>
                </div>
              ))}
            </div>
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

      {/* Modals */}
      <EditWordModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSave={handleSaveWord}
        initialWord={editingWord?.word || ''}
        initialActive={editingWord?.isActive ?? true}
        mode={modalMode}
      />

      <DeleteConfirmModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleConfirmDelete}
        wordToDelete={deletingWord?.word || ''}
      />
    </div>
  );
}
