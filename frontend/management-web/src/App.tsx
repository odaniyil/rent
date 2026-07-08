function App() {
  return (
    <main className="min-h-screen bg-slate-950 text-white">
      <section className="mx-auto flex min-h-screen max-w-5xl flex-col justify-center px-6 py-16">
        <p className="mb-4 text-sm font-semibold uppercase tracking-wide text-cyan-300">
          Rent-A-Home
        </p>
        <h1 className="text-4xl font-bold tracking-tight sm:text-6xl">
          Rent-A-Home Management Portal
        </h1>
        <p className="mt-6 max-w-2xl text-lg text-slate-300">
          Internal operations foundation for property onboarding, tenancy, field
          work, and rental administration.
        </p>
        <div className="mt-10 grid gap-4 sm:grid-cols-3">
          <div className="rounded-lg border border-slate-800 bg-slate-900 p-5">
            <h2 className="font-semibold">Owners</h2>
            <p className="mt-2 text-sm text-slate-400">
              Track leased apartments and owner relationships.
            </p>
          </div>
          <div className="rounded-lg border border-slate-800 bg-slate-900 p-5">
            <h2 className="font-semibold">Tenants</h2>
            <p className="mt-2 text-sm text-slate-400">
              Prepare for tenant lifecycle and rental workflows.
            </p>
          </div>
          <div className="rounded-lg border border-slate-800 bg-slate-900 p-5">
            <h2 className="font-semibold">Field Teams</h2>
            <p className="mt-2 text-sm text-slate-400">
              Support furnishing, inspections, and maintenance execution.
            </p>
          </div>
        </div>
      </section>
    </main>
  )
}

export default App
