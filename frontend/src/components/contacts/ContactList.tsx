import type { Contact } from '../../types';
import { ContactCard } from './ContactCard';

interface ContactListProps {
  contacts: Contact[];
  onEdit: (contact: Contact) => void;
  onDelete: (id: number) => void;
}

export function ContactList({ contacts, onEdit, onDelete }: ContactListProps) {
  if (!contacts || contacts.length === 0) {
    return (
      <div className="text-slate-500 text-center py-10 border-2 border-dashed border-slate-200 rounded-lg">
        No contacts found. Add someone to your phone book!
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-3">
      {contacts.map((contact) => (
        <ContactCard 
          key={contact.id} 
          contact={contact} 
          onEdit={onEdit} 
          onDelete={onDelete} 
        />
      ))}
    </div>
  );
}