export interface Contact {
    id: number;
    name: string;
    phoneNumber: string;
}
export interface CreateContactDto {
    name: string;
    phoneNumber: string;
}
export interface AiResponse {
    message: string;
    contact?: Contact;
}