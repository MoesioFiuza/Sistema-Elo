import { Header } from "./Header";

type PageLayoutProps = {
  children: React.ReactNode;
  backHref?: string;
  showLogin?: boolean;
  title: string;
  subtitle?: string;
  badge?: string;
  action?: React.ReactNode;
};

export function PageLayout({
  children,
  backHref = "/",
  showLogin = false,
  title,
  subtitle,
  badge,
  action,
}: PageLayoutProps) {
  return (
    <div className="min-h-screen">
      <Header showLogin={showLogin} backHref={backHref} />
      <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6">
        <div className="mb-8 flex flex-wrap items-end justify-between gap-4 animate-in">
          <div>
            {badge && (
              <p className="mb-1 text-xs font-semibold uppercase tracking-[0.15em] text-teal-700">
                {badge}
              </p>
            )}
            <h1 className="text-2xl font-bold tracking-tight text-[var(--elo-ink)] sm:text-3xl">
              {title}
            </h1>
            {subtitle && (
              <p className="mt-1.5 max-w-2xl text-sm text-[var(--elo-muted)]">{subtitle}</p>
            )}
          </div>
          {action}
        </div>
        <div className="animate-in">{children}</div>
      </main>
    </div>
  );
}
