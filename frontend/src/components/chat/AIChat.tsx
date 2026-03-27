import { useState, useRef, useEffect } from 'react';
import { api } from '../../services/api';
import type { Contact } from '../../types';

interface Message {
  id: string;
  text: string;
  isUser: boolean;
}

interface AIChatProps {
  onActionSuccess: () => Promise<void>;
  onContactSelect: (contact: Contact | null) => void;
}

export function AIChat({ onActionSuccess, onContactSelect }: AIChatProps) {
  const [messages, setMessages] = useState<Message[]>([
    { id: 'welcome', text: 'Hey, how can I help you? Try typing e.g. : "Add John with phone number 123-456-789".', isUser: false }
  ]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleSend = async () => {
    if (!input.trim() || isLoading) return;

    const userText = input.trim();
    setInput('');
    setMessages(prev => [...prev, { id: Date.now().toString(), text: userText, isUser: true }]);
    setIsLoading(true);

    try {
      const response = await api.askAI(userText);
      setMessages(prev => [...prev, { id: Date.now().toString(), text: response.message, isUser: false }]);
      
      if (response.contact !== undefined) {
        onContactSelect(response.contact);
      }
      await onActionSuccess(); 
    } catch (err: any) {
      setMessages(prev => [...prev, { id: Date.now().toString(), text: `${err.message}`, isUser: false }]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex flex-col h-full">
      <div className="flex-1 p-6 overflow-y-auto space-y-4">
        {messages.map((msg) => (
          <div 
            key={msg.id} 
            className={`flex ${msg.isUser ? 'justify-end' : 'justify-start'}`}
          >
            <div className={`max-w-[85%] p-3 rounded-2xl shadow-sm ${
              msg.isUser 
                ? 'bg-blue-600 text-white rounded-tr-none' 
                : 'bg-white text-slate-800 border border-slate-200 rounded-tl-none'
            }`}>
              {msg.text}
            </div>
          </div>
        ))}
        
        {isLoading && (
          <div className="flex justify-start">
            <div className="bg-slate-100 text-slate-500 border border-slate-200 p-3 rounded-2xl rounded-tl-none text-sm animate-pulse">
              AI is thinking...
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      <div className="p-4 bg-white border-t border-slate-200">
        <div className="flex gap-2">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSend()}
            placeholder="Type a command for the AI..."
            className="flex-1 border border-slate-300 rounded-xl px-4 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-all"
            disabled={isLoading}
          />
          <button
            onClick={handleSend}
            disabled={isLoading || !input.trim()}
            className="bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white px-5 py-2 rounded-xl font-medium transition-colors shadow-sm"
          >
            Send
          </button>
        </div>
      </div>
    </div>
  );
}