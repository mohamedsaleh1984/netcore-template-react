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
    if (data instanceof Array) {
        let items = "";
        data.forEach((element, index) => {
            items += `${index + 1}:${element}\n`
        });
        console.log(`called from ${funName} => ${items}`)
    }
    else {
        console.log(`called from ${funName} => ${data}`)
    }

}