import type { AiResponse, Contact, CreateContactDto } from '../types';

const API_BASE_URL = 'http://localhost:5207/api/contacts';

export const api = {
    async getAll(): Promise<Contact[]> {
        const response = await fetch(API_BASE_URL);
        if (!response.ok) throw new Error('Failed to fetch contacts');
        return response.json();
    },

    async add(contact: CreateContactDto): Promise<Contact> {
        const response = await fetch(API_BASE_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(contact),
        });
        if (!response.ok) throw new Error('Failed to add contact');
        return response.json();
    },

    async update(id: number, contact: CreateContactDto): Promise<void> {
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(contact),
        });
        if (!response.ok) throw new Error('Failed to update contact');
    },

    async delete(id: number): Promise<void> {
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE',
        });
        if (!response.ok) throw new Error('Failed to delete contact');
    },

    async askAI(command: string): Promise<AiResponse> {
        const response = await fetch(`${API_BASE_URL}/ask`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ commandInNaturalLanguage: command }),
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'AI request failed');
        }
        return response.json();
    }
};