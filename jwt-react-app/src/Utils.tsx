export const setAccessToken = (token: string) => {
    localStorage.setItem('accessToken', token);
};


export const removeAccessToken = () => {
    localStorage.removeItem('accessToken');
};

export const setRefreshToken = (token: string) => {
    localStorage.setItem('refreshToken', token);
};

export const removeRefreshToken = () => {
    localStorage.removeItem('refreshToken');
};

export const getRefreshToken = (): string => {
    return localStorage.getItem('refreshToken') || "";
}
export const getAccessToken = (): string => {
    return localStorage.getItem('accessToken') || "";
};

export const Inspect = (funName: string, data: any) => {
    console.log(`called from ${funName} => ${data}`)
}