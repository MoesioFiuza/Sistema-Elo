import Link from "next/link";

const accent: Record<string, string> = {
  "01": "border-l-teal-500",
  "02": "border-l-blue-500",
  "03": "border-l-amber-500",
  "04": "border-l-violet-500",
};

type ModuleCardProps = {
  titulo: string;
  descricao: string;
  perfil: string;
  href: string;
  indice: string;
};

export function ModuleCard({
  titulo,
  descricao,
  perfil,
  href,
  indice,
}: ModuleCardProps) {
  return (
    <Link
      href={href}
      className={`group block rounded-xl border border-slate-200 border-l-4 bg-white p-5 shadow-sm transition hover:border-slate-300 hover:shadow-md ${accent[indice] ?? "border-l-teal-500"}`}
    >
      <div className="mb-3 flex items-center justify-between">
        <span className="text-xs font-medium text-slate-400">{perfil}</span>
        <span className="font-mono text-xs text-slate-300">{indice}</span>
      </div>
      <h3 className="font-semibold text-slate-900 group-hover:text-teal-700">
        {titulo}
      </h3>
      <p className="mt-1.5 text-sm leading-relaxed text-slate-500">
        {descricao}
      </p>
    </Link>
  );
}
