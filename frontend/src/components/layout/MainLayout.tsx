import { useEffect, useState } from "react";
import type { Contact, CreateContactDto } from "../../types";
import { ContactList } from '../contacts/ContactList';
import { api } from "../../services/api";
import { ContactFormModal } from "../contacts/ContactFormModal";

export function MainLayout() {
  const [contacts, setContacts] = useState<Contact[]>([]);
  const [selectedContact, setSelectedContact] = useState<Contact | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchContacts();
  }, []);

  const fetchContacts = async () => {
    try {
      setIsLoading(true);
      const data = await api.getAll();
      setContacts(data);
      setError(null);
    } catch (err) {
      setError("Failed to load contacts. Is the backend running?");
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateNew = () => {
    setSelectedContact(null);
    setIsModalOpen(true);
  };

  const handleEdit = (contact: Contact) => {
    setSelectedContact(contact);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm("Are you sure you want to delete this contact?")) return;
    try {
      await api.delete(id);
      setContacts((prev) => prev.filter((c) => c.id !== id));
    } catch (err) {
      alert("Failed to delete contact. Please try again.");
    }
  };

  const handleFormSubmit = async (contactData: CreateContactDto) => {
    if (selectedContact) { // edit mode
      await api.update(selectedContact.id, contactData);
    } else {
      // create mode
      await api.add(contactData);
    }
    // refresh to get latest data
    await fetchContacts();
  };

  return (
    <div className="flex flex-col md:flex-row h-screen bg-slate-100 font-sans">
      
      {/* Left col: Phone Book */}
      <main className="w-full md:w-3/5 p-6 overflow-y-auto">
        <div className="bg-white rounded-xl shadow-sm p-6 min-h-full border border-slate-200">
          <header className="flex justify-between items-center mb-6">
            <h1 className="text-2xl font-bold text-slate-800">My contacts</h1>
            <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition-colors"
              onClick = {handleCreateNew}
            >
              + New Contact
            </button>
          </header>

          <ContactList 
            contacts={contacts}
            onEdit={handleEdit} 
            onDelete={handleDelete} 
          />
        </div>
      </main>

      {/* Right col: AI Chat */}
      <aside className="w-full md:w-2/5 bg-slate-50 border-l border-slate-200 flex flex-col">
        <div className="p-6 bg-white border-b border-slate-200 shadow-sm">
          <h2 className="text-xl font-bold text-slate-800 flex items-center gap-2">
            AI Assistant
          </h2>
          <p className="text-sm text-slate-500 mt-1">Manage contacts with natural language</p>
        </div>
        
        <div className="flex-1 p-6 overflow-y-auto">
          <div className="bg-blue-50 text-blue-800 p-4 rounded-lg rounded-tl-none inline-block max-w-[80%]">
            Hello! How can I help you today? Try typing: "Add John with phone number 123-456-789".
          </div>
        </div>

        <div className="p-4 bg-white border-t border-slate-200">
          <div className="text-slate-400 text-center text-sm">Type your message here...</div>
        </div>
      </aside>
      <ContactFormModal 
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleFormSubmit}
        initialData={selectedContact}
      />
    </div>
  );
}