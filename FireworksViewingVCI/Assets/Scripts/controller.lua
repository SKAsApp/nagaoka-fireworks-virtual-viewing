local Edamame = require "edamame"

-- Controllerクラス
local Controller = { }
Controller.new = function( )
	local instance = { }
	-- edamamemame: Edamame
	instance.__edamamemame = { }

	instance.initialise = function(self)
		for i = 1, 18 do
			self.__edamamemame[i] = Edamame.new(i)
		end
	end

	instance.on_edamame_use = function(self, index)
		self.__edamamemame[index]:push( )
	end

	instance.hit_on_mame = function(self, edamamemame_index, mame_index)
		-- 豆が頭へごちんしたら
		self.__edamamemame[edamamemame_index]:inform_hegochin(mame_index)
	end


	return instance
end
return Controller
