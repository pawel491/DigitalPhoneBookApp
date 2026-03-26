import type { Contact } from "../../types";


interface ContactCardProps {
  contact: Contact;
  onEdit: (contact: Contact) => void;
  onDelete: (id: number) => void;
}

export function ContactCard({ contact, onEdit, onDelete }: ContactCardProps) {
  return (
    <div className="bg-white border border-slate-200 rounded-lg p-4 flex items-center justify-between hover:shadow-md transition-shadow">
      
      <div className="flex items-center gap-4">
        <div className="w-10 h-10 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center font-bold text-lg">
          {contact.name.charAt(0).toUpperCase()}
        </div>
        <div>
          <h3 className="font-semibold text-slate-800">{contact.name}</h3>
          <p className="text-sm text-slate-500">{contact.phoneNumber}</p>
        </div>
      </div>

      <div className="flex gap-2">
        <button 
          onClick={() => onEdit(contact)}
          className="text-sm px-3 py-1.5 text-slate-600 bg-slate-100 hover:bg-slate-200 rounded-md transition-colors"
        >
          Edit
        </button>
        <button 
          onClick={() => onDelete(contact.id)}
          className="text-sm px-3 py-1.5 text-red-600 bg-red-50 hover:bg-red-100 rounded-md transition-colors"
        >
          Delete
        </button>
      </div>
      
    </div>
  );
}