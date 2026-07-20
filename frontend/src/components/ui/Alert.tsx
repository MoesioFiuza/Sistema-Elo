type AlertProps = {
  children: React.ReactNode;
  variant?: "error" | "success" | "info";
};

const styles = {
  error: "border-red-200 bg-red-50 text-red-800",
  success: "border-emerald-200 bg-emerald-50 text-emerald-800",
  info: "border-slate-200 bg-white text-slate-700",
};

export function Alert({ children, variant = "info" }: AlertProps) {
  return (
    <div className={`mb-6 rounded-lg border px-4 py-3 text-sm ${styles[variant]}`}>
      {children}
    </div>
  );
}
