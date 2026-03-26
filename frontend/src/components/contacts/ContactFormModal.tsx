import { useState, useEffect, type SubmitEvent } from 'react';
import { type Contact, type CreateContactDto } from '../../types';

interface ContactFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (contact: CreateContactDto) => Promise<void>;
  initialData?: Contact | null;
}

export function ContactFormModal({ isOpen, onClose, onSubmit, initialData }: ContactFormProps) {
  const [name, setName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    //edit mode
    if (initialData) {
      setName(initialData.name);
      setPhoneNumber(initialData.phoneNumber);
    } else { // create mode - empty form
      setName('');
      setPhoneNumber('');
    }
  }, [initialData, isOpen]);

  if (!isOpen) return null;

  const handleSubmit = async (e: SubmitEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!name.trim() || !phoneNumber.trim()) return;

    try {
      setIsSubmitting(true);
      await onSubmit({ name: name.trim(), phoneNumber: phoneNumber.trim() });
      onClose();
    } catch (error) {
      console.error(error);
      alert("Failed to save contact. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 backdrop-blur-md bg-slate-900/40">
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md p-6 border border-white/20">
        <h2 className="text-xl font-bold text-slate-800 mb-4">
          {initialData ? 'Edit Contact' : 'New Contact'}
        </h2>
        
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Name</label>
            <input 
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
              placeholder="e.g. John Doe"
            />
          </div>
          
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Phone Number</label>
            <input 
              type="text"
              required
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              className="w-full border border-slate-300 rounded-lg px-3 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
              placeholder="e.g. +1 234 567 890"
            />
          </div>

          <div className="flex justify-end gap-2 mt-4">
            <button 
              type="button" 
              onClick={onClose}
              disabled={isSubmitting}
              className="px-4 py-2 text-slate-600 bg-slate-100 hover:bg-slate-200 rounded-lg transition-colors disabled:opacity-50"
            >
              Cancel
            </button>
            <button 
              type="submit"
              disabled={isSubmitting}
              className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : 'Save'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}