import React, { useState, useEffect } from 'react';
import { X } from 'lucide-react';

interface EditWordModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (word: string, isActive: boolean) => Promise<void>;
  initialWord?: string;
  initialActive?: boolean;
  mode: 'add' | 'edit';
}

export const EditWordModal: React.FC<EditWordModalProps> = ({
  isOpen,
  onClose,
  onSave,
  initialWord = '',
  initialActive = true,
  mode
}) => {
  const [word, setWord] = useState(initialWord);
  const [isActive, setIsActive] = useState(initialActive);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      setWord(initialWord);
      setIsActive(initialActive);
      setError(null);
    }
  }, [isOpen, initialWord, initialActive]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!word.trim()) {
      setError('Word cannot be empty');
      return;
    }

    setIsSaving(true);
    setError(null);

    try {
      await onSave(word.trim(), isActive);
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save word');
    } finally {
      setIsSaving(false);
    }
  };

  const handleClose = () => {
    if (!isSaving) {
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-lime-600 to-lime-700">
          <h2 className="text-xl font-semibold text-white">
            {mode === 'add' ? 'Add New Word' : 'Edit Word'}
          </h2>
          <button
            onClick={handleClose}
            disabled={isSaving}
            className="text-white hover:text-gray-200 transition-colors disabled:opacity-50"
            aria-label="Close"
          >
            <X size={24} />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6">
          {error && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
              <p className="text-sm text-red-600">{error}</p>
            </div>
          )}

          <div className="space-y-4">
            {/* Word Input */}
            <div>
              <label htmlFor="word" className="block text-sm font-medium text-gray-700 mb-1">
                Word
              </label>
              <input
                id="word"
                type="text"
                value={word}
                onChange={(e) => setWord(e.target.value)}
                disabled={isSaving}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-lime-500 focus:border-transparent disabled:bg-gray-100 disabled:cursor-not-allowed"
                placeholder="Enter sensitive word"
                autoFocus
              />
            </div>

            {/* Active Status Toggle */}
            <div className="flex items-center justify-between">
              <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
                Status
              </label>
              <div className="flex items-center space-x-3">
                <span className={`text-sm ${isActive ? 'text-gray-500' : 'font-semibold text-gray-700'}`}>
                  Inactive
                </span>
                <button
                  type="button"
                  id="isActive"
                  onClick={() => setIsActive(!isActive)}
                  disabled={isSaving}
                  className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors focus:outline-none focus:ring-2 focus:ring-lime-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed ${
                    isActive ? 'bg-lime-600' : 'bg-gray-300'
                  }`}
                  aria-pressed={isActive}
                >
                  <span
                    className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                      isActive ? 'translate-x-6' : 'translate-x-1'
                    }`}
                  />
                </button>
                <span className={`text-sm ${isActive ? 'font-semibold text-gray-700' : 'text-gray-500'}`}>
                  Active
                </span>
              </div>
            </div>
          </div>

          {/* Footer Buttons */}
          <div className="flex justify-end space-x-3 mt-6">
            <button
              type="button"
              onClick={handleClose}
              disabled={isSaving}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSaving}
              className="px-4 py-2 text-sm font-medium text-white bg-lime-600 rounded-md hover:bg-lime-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isSaving ? 'Saving...' : mode === 'add' ? 'Add Word' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
