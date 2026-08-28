local Mame = require "mame"

-- Edamameクラス
local Edamame = { }
Edamame.new = function(index)
	local instance = { }
	instance.__index = index
	-- ここでinitialiseはおかしくない？
	instance.__mamemame = { }
	instance.__count = 0

	instance.initialise = function(self, index)
		for i = 1, 3 do
			self.__edamamemame[i] = Mame.new(index, i)
		end
	end

	instance.push = function(self)
		-- 押されたら豆を飛び出させる
		if self.__count >= 0 and self.__count < 3 then
			self.__count = self.__count + 1
			self.__mamemame[self.__count]:out( )
		end
		if self.__count > 3 then
			self.__mamemame[self.__count]:go_back( )
			self.__count = 0
		end
	end

	instance.inform_hegochin = function(self, mame_index)
		-- へごちんされてることをお豆さんに伝える
		self.__mamemame[mame_index]:eaten( )
	end

	instance:initialise(index)
	return instance
end
return Edamame
