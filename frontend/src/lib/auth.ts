"use client";

export type PerfilUsuario = "Medico" | "Laboratorio" | "CCIH" | "Enfermagem" | "Admin";

export type AuthUser = {
  usuarioId: string;
  nome: string;
  email: string;
  perfil: PerfilUsuario;
  token: string;
  expiraEm: string;
};

const KEY = "elo.auth";

export function getAuth(): AuthUser | null {
  if (typeof window === "undefined") return null;
  try {
    const raw = localStorage.getItem(KEY);
    if (!raw) return null;
    const data = JSON.parse(raw) as AuthUser;
    if (new Date(data.expiraEm) < new Date()) {
      localStorage.removeItem(KEY);
      return null;
    }
    return data;
  } catch {
    return null;
  }
}

export function setAuth(user: AuthUser) {
  localStorage.setItem(KEY, JSON.stringify(user));
}

export function clearAuth() {
  localStorage.removeItem(KEY);
}

export function authHeader(): Record<string, string> {
  const auth = getAuth();
  return auth ? { Authorization: `Bearer ${auth.token}` } : {};
}

export function homeForPerfil(perfil: PerfilUsuario): string {
  switch (perfil) {
    case "Medico":
      return "/";
    case "Laboratorio":
      return "/laboratorio";
    case "CCIH":
    case "Enfermagem":
      return "/ccih";
    case "Admin":
      return "/";
    default:
      return "/";
  }
}
