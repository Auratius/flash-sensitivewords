import React from 'react';
import { AlertTriangle, X } from 'lucide-react';

interface DeleteConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => Promise<void>;
  wordToDelete: string;
}

export const DeleteConfirmModal: React.FC<DeleteConfirmModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
  wordToDelete
}) => {
  const [isDeleting, setIsDeleting] = React.useState(false);

  const handleConfirm = async () => {
    setIsDeleting(true);
    try {
      await onConfirm();
      onClose();
    } catch (err) {
      console.error('Failed to delete word:', err);
    } finally {
      setIsDeleting(false);
    }
  };

  const handleClose = () => {
    if (!isDeleting) {
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-red-600 to-red-700">
          <div className="flex items-center space-x-2">
            <AlertTriangle className="text-white" size={24} />
            <h2 className="text-xl font-semibold text-white">Confirm Delete</h2>
          </div>
          <button
            onClick={handleClose}
            disabled={isDeleting}
            className="text-white hover:text-gray-200 transition-colors disabled:opacity-50"
            aria-label="Close"
          >
            <X size={24} />
          </button>
        </div>

        {/* Content */}
        <div className="p-6">
          <p className="text-gray-700 mb-2">
            Are you sure you want to delete this word?
          </p>
          <div className="bg-gray-50 border border-gray-200 rounded-md p-3 my-4">
            <p className="text-center font-semibold text-gray-900">"{wordToDelete}"</p>
          </div>
          <p className="text-sm text-gray-600">
            This action cannot be undone. The word will be permanently removed from the system.
          </p>
        </div>

        {/* Footer Buttons */}
        <div className="flex justify-end space-x-3 px-6 pb-6">
          <button
            type="button"
            onClick={handleClose}
            disabled={isDeleting}
            className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Cancel
          </button>
          <button
            type="button"
            onClick={handleConfirm}
            disabled={isDeleting}
            className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-md hover:bg-red-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isDeleting ? 'Deleting...' : 'Delete Word'}
          </button>
        </div>
      </div>
    </div>
  );
};
