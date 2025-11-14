import { useState } from 'react';
import { MessageSquare, Database } from 'lucide-react';
import { SanitizeForm } from './components/SanitizeForm';
import { WordsManager } from './components/WordsManager';
import { HealthStats } from './components/HealthStats';
import { PageGuide } from './components/PageGuide';

function App() {
  const [activeTab, setActiveTab] = useState<'sanitize' | 'words'>('sanitize');

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 flex flex-col">
      <div className="container mx-auto px-4 py-2 max-w-8xl flex-1 flex flex-col">
        <header className="text-center mb-4">
          <div className="mb-2">
            <div className="p-3 bg-lime-600 rounded-lg flex items-center justify-center gap-7 relative">
              <img src="https://cdn.prod.website-files.com/642b08d9f919f4a6470dea8f/6436947376dd6e87c5b84177_logo.svg" loading="lazy" id="76595" alt="Untitled UI logotext" className="navbar_logo absolute left-3" />
              <h1 className="text-4xl font-bold text-black-400" style={{ fontFamily: 'Inter, system-ui, -apple-system, sans-serif', letterSpacing: '-0.02em' }}>
                SQL Sensitive Word Sanitizer
              </h1>
            </div>
          </div>
        </header>

        <div className="mb-4">
          <HealthStats />
        </div>

        {/* Tabs Navigation */}
        <div className="bg-white rounded-lg shadow-md overflow-hidden mb-8 flex-1 flex flex-col">
          <div className="border-b border-gray-200">
            <div className="flex">
              <button
                onClick={() => setActiveTab('sanitize')}
                className={`flex-1 flex items-center justify-center gap-2 px-4 py-3 text-sm font-semibold transition-colors ${
                  activeTab === 'sanitize'
                    ? 'bg-lime-600 text-white border-b-2 border-lime-700'
                    : 'bg-gray-50 text-gray-600 hover:bg-gray-100'
                }`}
              >
                <MessageSquare className="w-4 h-4" />
                <span>Message Sanitizer</span>
              </button>
              <button
                onClick={() => setActiveTab('words')}
                className={`flex-1 flex items-center justify-center gap-2 px-4 py-3 text-sm font-semibold transition-colors ${
                  activeTab === 'words'
                    ? 'bg-lime-600 text-white border-b-2 border-lime-700'
                    : 'bg-gray-50 text-gray-600 hover:bg-gray-100'
                }`}
              >
                <Database className="w-4 h-4" />
                <span>Words Manager</span>
              </button>
            </div>
          </div>

          {/* Tab Content */}
          <div className="p-6 flex-1 overflow-auto">
            {activeTab === 'sanitize' && <SanitizeForm />}
            {activeTab === 'words' && <WordsManager />}
          </div>
        </div>

        <footer className="mt-auto">
          <div className="p-3 bg-lime-600 rounded-lg">
            <div className="text-center text-sm text-white">
              <p className="mb-2">
                <strong>API Base URL:</strong>{' '}
                <code className="px-2 py-1 bg-lime-700 rounded text-xs">
                  {import.meta.env.VITE_API_BASE_URL || 'https://localhost:7001/api'}
                </code>
              </p>
              <p>Built with DDD, CQRS, and Microservice Architecture</p>
            </div>
          </div>
        </footer>
      </div>

      {/* Floating Help Guide */}
      <PageGuide />
    </div>
  );
}

export default App;
