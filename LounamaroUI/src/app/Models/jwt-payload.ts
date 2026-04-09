export interface JwtPayload {
  exp: number;
  // Use index signature to allow dynamic keys
  [key: string]: any;
}
