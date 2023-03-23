

export default interface ISessionDto {
    id: string;
    ip: string | null;
    userAgent: string | null;
    createAt: string;
}