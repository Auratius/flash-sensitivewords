# Flash.SensitiveWords React UI

A clean, modern web interface for testing and managing SQL keyword sanitization. Built with React and TypeScript.

## What it does

This gives you a friendly UI to:
- Test message sanitization in real-time
- Manage your list of sensitive words
- Monitor system health
- See how the API works

## Quick Start

```bash
# Install dependencies
npm install

# Start dev server
npm run dev
```

Then open http://localhost:5173 in your browser.

**Note:** Make sure the API is running first at https://localhost:64725

Or just use our script that starts everything:
```bash
# From project root
.\SpecialScripts\start-fullstack-windows.bat
```

## What's in the UI

### Message Sanitizer Tab
Type in a message and see what SQL keywords get caught. For example:
- "SELECT * FROM users" becomes "****** * **** users"
- Shows you which keywords were found
- Lets you copy the results

### Words Manager Tab
Manage your database of sensitive words:
- See all the words (shows 7 at a time, scroll for more)
- Add new words like "TRUNCATE" or "GRANT"
- Toggle words on/off without deleting them
- Delete words you don't need anymore
- Search to find specific words

### Health Stats (Top of Page)
Shows you what's happening with the API:
- How long it's been running
- Memory usage
- CPU time
- Active threads
- Updates every 5 seconds automatically

### Help Guide (Bottom Right)
Click the green button in the corner for help with anything.

## What you need

- Node.js 18 or newer
- npm (comes with Node)
- The API running (see above)

## Available Commands

```bash
# Start development server with hot reload
npm run dev

# Build for production
npm run build

# Preview production build locally
npm run preview

# Check types
npm run type-check
```

## How it's built

Pretty standard React setup:
- **React 18** - The UI framework
- **TypeScript** - Keeps us from making dumb mistakes
- **Vite** - Super fast dev server and build tool
- **Tailwind CSS** - For styling (the lime green is our brand color)
- **Lucide Icons** - Nice looking icons

## Project Structure

```
src/
├── components/
│   ├── SanitizeForm.tsx      # Message testing interface
│   ├── WordsManager.tsx       # Word management interface
│   ├── HealthStats.tsx        # System monitoring
│   └── PageGuide.tsx          # Help system
├── services/
│   └── api.ts                 # API calls
├── types/
│   └── index.ts               # TypeScript types
├── App.tsx                    # Main app with tabs
└── main.tsx                   # Entry point
```

Everything's organized by what it does, not by file type. Makes it easier to find stuff.

## Configuration

Create a `.env` file if you need to change the API URL:

```env
VITE_API_BASE_URL=https://localhost:64725/api
```

But honestly, the default works fine for local development.

## Common Issues

**API connection errors?**
- Make sure the API is running (check https://localhost:64725/swagger)
- Check the console in your browser for detailed errors

**Port 5173 already in use?**
```bash
npm run dev -- --port 3000
```

**Slow loading?**
- First run always takes longer (Vite is setting things up)
- Clear the cache: delete `node_modules/.vite` folder

**Build fails?**
```bash
# Clean install
rm -rf node_modules package-lock.json
npm install
```

## Design Notes

We went with a clean, professional look:
- Lime green (#65a30d) for primary actions
- Yellow (#facc15) for the title (matches the Flash branding)
- Gray for most text and backgrounds
- Red for errors, green for success

The layout uses tabs because it's cleaner than having everything on one page. Footer stays at the bottom always, and the Words Manager shows exactly 7 items so it doesn't get overwhelming.

## Making Changes

The code's pretty straightforward:
1. Components are in `src/components/`
2. API calls are in `src/services/api.ts`
3. Types are in `src/types/index.ts`
4. Main layout is in `src/App.tsx`

Everything uses TypeScript, so your editor will yell at you if you mess something up. That's a good thing.

Hot reload works great - just save and your browser updates automatically.

## Deploying

Build it:
```bash
npm run build
```

Upload the `dist/` folder to wherever you host static sites:
- Netlify
- Vercel
- AWS S3
- Azure Static Web Apps
- Any static file hosting really

Just remember to set `VITE_API_BASE_URL` to your production API URL.

## Tech Stack

- React 18.3
- TypeScript 5.5
- Vite 5.4
- Tailwind CSS 3.4
- Lucide React (icons)
- Inter font (from Google Fonts)

That's it. We kept dependencies minimal on purpose.
