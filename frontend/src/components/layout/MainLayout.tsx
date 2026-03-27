import { useEffect, useState } from "react";
import type { Contact, CreateContactDto } from "../../types";
import { ContactList } from '../contacts/ContactList';
import { api } from "../../services/api";
import { ContactFormModal } from "../contacts/ContactFormModal";
import { AIChat } from "../chat/AIChat";
import { useContacts } from "../../hooks/useContacts";

export function MainLayout() {
  const { contacts, isLoading, error, fetchContacts, addOrUpdateContact, deleteContact } = useContacts();
  
  const [selectedContact, setSelectedContact] = useState<Contact | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  // Auto scroll to selected contact
  useEffect(() => {
    if(selectedContact) {
      const element = document.getElementById(`contact-${selectedContact.id}`);
      if(element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    }
  }, [selectedContact, contacts]);


  const handleContactSelect = (contact: Contact) => {
    setSelectedContact(contact);
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
    const isSuccess = await deleteContact(id);
    if (isSuccess && selectedContact?.id === id) {
      setSelectedContact(null);
    }
  };

  const handleFormSubmit = async (contactData: CreateContactDto) => {
    try {
      const newContact = await addOrUpdateContact(contactData, selectedContact?.id);
      if(newContact) {
        setSelectedContact(newContact);
      } 
    } catch (err) {
      console.error(err);
    }
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

          {error ? (
            <div className="text-red-500 text-center py-10 bg-red-50 rounded-lg border border-red-200">
              {error}
            </div>
          ) : isLoading ? (
            <div className="flex flex-col items-center justify-center py-20 text-slate-400">
              <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600 mb-4"></div>
              <p>Fetching contacts...</p>
            </div>
          ) : (
            <ContactList 
              contacts={contacts}
              onEdit={handleEdit} 
              onDelete={handleDelete} 
              selectedId={selectedContact?.id}
              onSelect={handleContactSelect}
            />
          ) }
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
        
        <div className="flex-1 overflow-hidden">
          <AIChat 
            onActionSuccess={fetchContacts} 
            onContactSelect={setSelectedContact} 
          />
        </div>
      </aside>
      <ContactFormModal 
        isOpen={isModalOpen}
        onClose={() => { 
          setIsModalOpen(false);
        }} 
        onSubmit={handleFormSubmit}
        initialData={selectedContact}
      />
    </div>
  );
}