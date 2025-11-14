import { useState } from 'react';
import { X, Info, MessageSquare, Database, Activity, Zap } from 'lucide-react';

interface GuideSection {
  id: string;
  icon: React.ReactNode;
  title: string;
  description: string;
  tips: string[];
}

const guideSections: GuideSection[] = [
  {
    id: 'sanitize',
    icon: <MessageSquare className="w-5 h-5" />,
    title: 'Message Sanitizer',
    description: 'Test SQL keyword detection and sanitization in real-time.',
    tips: [
      'Enter any text containing SQL keywords like SELECT, DELETE, DROP, etc.',
      'Click "Sanitize Message" to see detected keywords replaced with asterisks',
      'View the count of detected and replaced keywords',
      'Perfect for testing your message filters'
    ]
  },
  {
    id: 'words',
    icon: <Database className="w-5 h-5" />,
    title: 'Sensitive Words Manager',
    description: 'Manage the database of sensitive SQL keywords.',
    tips: [
      'View all sensitive words currently in the database',
      'Add new SQL keywords to block (e.g., TRUNCATE, GRANT)',
      'Toggle words active/inactive without deleting them',
      'Delete words permanently from the database',
      'Search and filter words in the list'
    ]
  },
  {
    id: 'health',
    icon: <Activity className="w-5 h-5" />,
    title: 'Health & Performance Stats',
    description: 'Monitor API health and performance metrics.',
    tips: [
      'Green status indicates API is healthy and responsive',
      'View total requests processed',
      'Check average response time',
      'Monitor active sanitization operations',
      'See total sensitive words in database'
    ]
  },
  {
    id: 'quickstart',
    icon: <Zap className="w-5 h-5" />,
    title: 'Quick Start',
    description: 'Get started in 3 easy steps:',
    tips: [
      '1. Check that Health Stats shows "Healthy" status',
      '2. Try sanitizing a test message: "SELECT * FROM users WHERE id=1"',
      '3. Add or manage sensitive words in the Words Manager',
      'Use the API endpoint shown in the footer for integration'
    ]
  }
];

export function PageGuide() {
  const [isOpen, setIsOpen] = useState(false);
  const [activeSection, setActiveSection] = useState<string>('quickstart');

  if (!isOpen) {
    return (
      <button
        onClick={() => setIsOpen(true)}
        className="fixed bottom-6 right-6 bg-lime-600 text-white p-4 rounded-full shadow-lg hover:bg-lime-700 transition-all duration-200 hover:scale-110 z-50 group"
        title="Show Page Guide"
      >
        <Info className="w-6 h-6" />
        <span className="absolute -top-12 right-0 bg-gray-800 text-white text-sm px-3 py-1 rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap">
          Need help? Click for guide
        </span>
      </button>
    );
  }

  const currentSection = guideSections.find(s => s.id === activeSection) || guideSections[0];

  return (
    <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-xl shadow-2xl max-w-3xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        {/* Header */}
        <div className="bg-gradient-to-r from-lime-600 to-lime-700 text-white p-6">
          <div className="flex items-center justify-between mb-2">
            <div className="flex items-center gap-3">
              <div className="bg-white/20 p-2 rounded-lg">
                <Info className="w-6 h-6" />
              </div>
              <h2 className="text-2xl font-bold">Page Guide</h2>
            </div>
            <button
              onClick={() => setIsOpen(false)}
              className="hover:bg-white/20 p-2 rounded-lg transition-colors"
              title="Close Guide"
            >
              <X className="w-6 h-6" />
            </button>
          </div>
          <p className="text-lime-100">Learn how to use the SQL Sanitize Microservice</p>
        </div>

        {/* Navigation Tabs */}
        <div className="border-b border-gray-200 bg-gray-50">
          <div className="flex overflow-x-auto">
            {guideSections.map((section) => (
              <button
                key={section.id}
                onClick={() => setActiveSection(section.id)}
                className={`flex items-center gap-2 px-6 py-3 font-medium transition-colors whitespace-nowrap ${
                  activeSection === section.id
                    ? 'text-lime-600 border-b-2 border-lime-600 bg-white'
                    : 'text-gray-600 hover:text-lime-600 hover:bg-white/50'
                }`}
              >
                {section.icon}
                {section.title}
              </button>
            ))}
          </div>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-6">
          <div className="space-y-4">
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-2 flex items-center gap-2">
                {currentSection.icon}
                {currentSection.title}
              </h3>
              <p className="text-gray-600 mb-4">{currentSection.description}</p>
            </div>

            <div className="bg-lime-50 border border-lime-200 rounded-lg p-4">
              <h4 className="font-semibold text-lime-800 mb-3 flex items-center gap-2">
                <Zap className="w-4 h-4" />
                {currentSection.id === 'quickstart' ? 'Steps:' : 'How to use:'}
              </h4>
              <ul className="space-y-2">
                {currentSection.tips.map((tip, index) => (
                  <li key={index} className="flex items-start gap-2 text-gray-700">
                    <span className="text-lime-600 font-bold mt-0.5">
                      {currentSection.id === 'quickstart' ? '→' : '•'}
                    </span>
                    <span>{tip}</span>
                  </li>
                ))}
              </ul>
            </div>

            {/* Additional Info Cards */}
            {activeSection === 'sanitize' && (
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                <h4 className="font-semibold text-blue-800 mb-2">Example Usage:</h4>
                <div className="space-y-2 text-sm">
                  <div className="bg-white p-3 rounded border border-blue-200">
                    <div className="font-mono text-gray-700">
                      <strong>Input:</strong> "SELECT * FROM users WHERE password='admin'"
                    </div>
                    <div className="font-mono text-gray-700 mt-2">
                      <strong>Output:</strong> "****** * **** users WHERE password='admin'"
                    </div>
                    <div className="text-blue-600 mt-2">
                      <strong>Detected:</strong> SELECT, FROM (2 keywords)
                    </div>
                  </div>
                </div>
              </div>
            )}

            {activeSection === 'words' && (
              <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
                <h4 className="font-semibold text-yellow-800 mb-2">⚠️ Important:</h4>
                <ul className="space-y-1 text-sm text-gray-700">
                  <li>• Words are automatically converted to UPPERCASE</li>
                  <li>• Duplicate words are rejected by the database</li>
                  <li>• Deactivated words won't be used in sanitization</li>
                  <li>• Deleting a word is permanent (use deactivate for temporary)</li>
                </ul>
              </div>
            )}

            {activeSection === 'health' && (
              <div className="grid grid-cols-2 gap-4">
                <div className="bg-green-50 border border-green-200 rounded-lg p-3">
                  <div className="text-green-600 font-semibold mb-1">✓ Healthy</div>
                  <div className="text-sm text-gray-600">API is responding normally</div>
                </div>
                <div className="bg-red-50 border border-red-200 rounded-lg p-3">
                  <div className="text-red-600 font-semibold mb-1">✗ Unhealthy</div>
                  <div className="text-sm text-gray-600">API is unreachable or failing</div>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Footer */}
        <div className="border-t border-gray-200 bg-gray-50 p-4 flex items-center justify-between">
          <div className="text-sm text-gray-600">
            Built with DDD, CQRS, and Microservice Architecture
          </div>
          <button
            onClick={() => setIsOpen(false)}
            className="px-4 py-2 bg-lime-600 text-white rounded-lg hover:bg-lime-700 transition-colors font-medium"
          >
            Got it, thanks!
          </button>
        </div>
      </div>
    </div>
  );
}
