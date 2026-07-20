type StatCardProps = {
  label: string;
  value: number | string;
  tone?: "default" | "warning" | "danger" | "success";
};

const tones = {
  default: "bg-white",
  warning: "bg-amber-50 ring-amber-100",
  danger: "bg-red-50 ring-red-100",
  success: "bg-emerald-50 ring-emerald-100",
};

const values = {
  default: "text-slate-900",
  warning: "text-amber-800",
  danger: "text-red-800",
  success: "text-emerald-800",
};

export function StatCard({ label, value, tone = "default" }: StatCardProps) {
  return (
    <div
      className={`rounded-xl border border-slate-200 p-5 shadow-sm ring-1 ring-inset ${tones[tone]}`}
    >
      <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
        {label}
      </p>
      <p className={`mt-2 text-3xl font-bold tabular-nums ${values[tone]}`}>
        {value}
      </p>
    </div>
  );
}
