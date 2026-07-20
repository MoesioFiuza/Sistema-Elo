import type { NextConfig } from "next";

const API_URL = process.env.API_INTERNAL_URL ?? "http://127.0.0.1:5000/api/v1";

const nextConfig: NextConfig = {
  output: "standalone",
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: `${API_URL}/:path*`,
      },
    ];
  },
};

export default nextConfig;
