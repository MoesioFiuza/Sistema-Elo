import Link from "next/link";

type BrandMarkProps = {
  href?: string;
  compact?: boolean;
};

export function BrandMark({ href = "/", compact = false }: BrandMarkProps) {
  const content = (
    <span className="flex items-center gap-2.5">
      {/* eslint-disable-next-line @next/next/no-img-element */}
      <img
        src="/nepec-logo.svg"
        alt="NEPEC"
        className={compact ? "h-9 w-auto" : "h-10 w-auto"}
      />
      <span className="leading-tight">
        <span className="block font-semibold tracking-tight text-[var(--elo-ink)]">
          Cdigital
        </span>
        {!compact && (
          <span className="hidden text-[10px] uppercase tracking-wider text-[var(--elo-muted)] sm:block">
            Plataforma de pesquisa · NEPEC
          </span>
        )}
      </span>
    </span>
  );

  if (!href) return content;
  return (
    <Link href={href} className="flex items-center gap-2.5">
      {content}
    </Link>
  );
}
