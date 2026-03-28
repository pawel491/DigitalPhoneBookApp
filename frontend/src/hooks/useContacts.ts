import { useState, useEffect, useCallback } from "react";
import { api } from "../services/api";
import type { Contact, CreateContactDto } from "../types";

export function useContacts() {
    const [contacts, setContacts] = useState<Contact[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchContacts = useCallback(async () => {
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
    }, []);

    // initial load
    useEffect(() => {
        fetchContacts();
    }, [fetchContacts]);

    const addOrUpdateContact = async (
        contactData: CreateContactDto,
        selectedContactId?: number
    ): Promise<Contact | null> => {
        // passes possible exceptions to caller for better handling
        if (selectedContactId) { // edit mode
            await api.update(selectedContactId, contactData);
            await fetchContacts();
            return null;
        } else { // create mode
            const newContact = await api.add(contactData);
            await fetchContacts();
            return newContact;
        }
    };

    const deleteContact = async (id: number) => {
        if (!window.confirm("Are you sure you want to delete this contact?")) return false;
        try {
            await api.delete(id);
            setContacts((prev) => prev.filter((c) => c.id !== id));
            return true;
        } catch (err) {
            alert("Failed to delete contact. Please try again.");
            return false;
        }
    };

    return {
        contacts,
        isLoading,
        error,
        fetchContacts,
        addOrUpdateContact,
        deleteContact
    };
}