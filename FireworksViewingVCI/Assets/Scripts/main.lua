print("Controller読み込み")
local Controller = require "controller"
local controller = Controller.new( )
controller:initialise( )
print("controllerを生成し，初期化しました。")


onUse = function(use)
	-- （VCIのイベントハンドラー）グリップしたとき
	-- param use: グリップしたSubItemの名前
	print(use .. "が押されました。")
	if string.match(use, "Edamame %(([0-9]{1,})%)") then
		for index_string in string.gmatch(use, "Edamame %(([0-9]{1,})%)") do
			controller:on_edamame_use(tonumber(index_string))
		end
	end
end


onCollisionEnter = function(item, hit)
	-- TODO 正規表現を使って、豆を特定する
	if string.match(item, "Mame %(([0-9]{1,})-%([0-9]{1,}))") then
		for edamamemame_index_string, mame_index_string in string.gmatch(use, "Edamame %(([0-9]{1,})%)") do
			controller:on_edamame_use(tonumber(index_string))
		end
	end

	if item == "Mame (1-1)" and hit == "Head" then
		controller:hit_on_mame(1, 1)
		return
	end
end


-- updateAll = function( )
-- 	-- （VCIのイベントハンドラー）全ユーザーでの毎フレーム実行
-- 	controller:update( )
-- 	return
-- end
