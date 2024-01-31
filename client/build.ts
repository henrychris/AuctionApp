// wtf is going on

await Bun.build({
  entrypoints: [
    "./public/scripts/ts/index.ts",
    "./public/scripts/ts/auth.ts",
    "./public/scripts/ts/config.ts",
    "./public/scripts/ts/helper.ts",
    "./public/scripts/ts/rooms.ts",
    "./public/scripts/ts/signalRConn.ts",
    "./public/scripts/ts/api.ts",
    "./public/scripts/ts/payment.ts",],
  outdir: "./public/scripts/js/",
});
